using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QmsCore.Model;
using QmsCore.Engine;

namespace QmsCore.Validation
{
    public class EmployeeValidator : IValidator
    {
        QMSContext context;

        List<QmsEmployee> existingEmployees;  


        List<QmsEmployee> newEmployees;

        bool UseSoftDelete {get;set;}

        public IList RecordsToDelete {get;set;}
        public IList RecordsToInsert {get;set;}
        public IList RecordsToUpdate {get;set;}

        public EmployeeValidator()
        {
            context = new QMSContext();
        }

        public void Validate(IList records, bool useSoftDelete)
        {
            try
            {
                UseSoftDelete = useSoftDelete;
                newEmployees = (List<QmsEmployee>)records;
                setIdentifiersOnNewEmployees();
                RecordsToDelete = new List<QmsEmployee>();
                RecordsToInsert = new List<QmsEmployee>();
                RecordsToUpdate = new List<QmsEmployee>();
                loadExistingRecords();
                setEmployeesForInsert();
                setEmployeesForDeletionOrUpdate();                
            }
            catch (System.Exception x)
            {
                Logger.Log.Record(LogType.Error,x.ToString());
            }

        }

        private void setIdentifiersOnNewEmployees()
        {
            foreach(QmsEmployee emp in newEmployees)
            {
                emp.EmplId = padEmplId(emp.EmplId);
                emp.UserKey = Security.GetHashedValue(emp.Ssn);
            }
        }

        private string padEmplId(string emplId)
        {
            string retval = emplId;
            int correctLength = 8;
            int currentLength = retval.Length;
            while(currentLength < correctLength)
            {
                retval = "0" + retval;
                currentLength = retval.Length;
            }
            return retval;
        }

        void loadExistingRecords()
        {
            
            //existingEmployees = context.QmsEmployee.Where(e => e.DeletedAt == null).ToList();
            existingEmployees = QmsCore.Data.Context.QmsEmployee.ToList();
        }

        void setEmployeesForInsert()
        {            
            foreach(QmsEmployee newEmployee in newEmployees)
            {
                int i = existingEmployees.Count(e => e.EmplId == newEmployee.EmplId);
                if(i == 0)
                {
                    RecordsToInsert.Add(newEmployee);
                    Logger.Log.Record(string.Format("Employee {0} {1},{2} to be inserted",newEmployee.EmplId,newEmployee.LastName,newEmployee.FirstName));
                }
            }
            Logger.Log.Record(RecordsToInsert.Count  + " records to be added to the database.");
        }

        void setEmployeesForDeletionOrUpdate()
        {
            foreach(QmsEmployee existingEmployee in existingEmployees)
            {
                QmsEmployee emp = newEmployees.Where(e => e.EmplId == existingEmployee.EmplId).SingleOrDefault();
                if(emp!=null)
                {
                    int updatedFieldCount = 0;
                    if(existingEmployee.FirstName != emp.FirstName)
                    {
                        existingEmployee.FirstName = emp.FirstName;    
                        updatedFieldCount++;
                    }
                    if(existingEmployee.LastName != emp.LastName)
                    {
                        existingEmployee.LastName = emp.LastName;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.MiddleName != emp.MiddleName)
                    {
                        existingEmployee.MiddleName = emp.MiddleName;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.DepartmentId != emp.DepartmentId)
                    {
                        existingEmployee.DepartmentId = emp.DepartmentId;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.EmailAddress != emp.EmailAddress)
                    {
                        existingEmployee.EmailAddress = emp.EmailAddress;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.AgencySubElement != emp.AgencySubElement)
                    {
                        existingEmployee.AgencySubElement = emp.AgencySubElement;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.PersonnelOfficeIdentifier != emp.PersonnelOfficeIdentifier)
                    {
                        existingEmployee.PersonnelOfficeIdentifier = emp.PersonnelOfficeIdentifier;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.Grade != emp.Grade)
                    {
                        existingEmployee.Grade = emp.Grade;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.PayPlan != emp.PayPlan)
                    {
                        existingEmployee.PayPlan = emp.PayPlan;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.ManagerId != emp.ManagerId)
                    {
                        existingEmployee.ManagerId = emp.ManagerId;
                        updatedFieldCount++;
                    }
                    if(existingEmployee.UserKey != emp.UserKey)
                    {
                        existingEmployee.UserKey = emp.UserKey;
                        updatedFieldCount++;
                    }

                    if (updatedFieldCount > 0)
                    {
                        existingEmployee.UpdatedAt =DateTime.Now;
                        RecordsToUpdate.Add(existingEmployee);
                        Logger.Log.Record(string.Format("Employee {0} {1},{2} has {3} field(s) that were updated",existingEmployee.EmplId,existingEmployee.LastName,existingEmployee.FirstName,updatedFieldCount));                         
                    }
                }
                else //employee in database but not in csv file so we delete them
                {
                    existingEmployee.DeletedAt = DateTime.Now;
                    existingEmployee.UpdatedAt = DateTime.Now;
                    RecordsToDelete.Add(existingEmployee);
                    Logger.Log.Record(string.Format("Employee {0} {1},{2} to be deleted",existingEmployee.EmplId,existingEmployee.LastName,existingEmployee.FirstName));                    
                }
            }
            Logger.Log.Record(RecordsToUpdate.Count  + " records to be updated in the database.");
            Logger.Log.Record(RecordsToDelete.Count  + " records to be deleted in the database.");                
        }

        public void Save()
        {
            insert();
            update();
            delete();
        }

        private void delete()
        {
            int i = RecordsToDelete.Count;
            int j = 0;
            if(i > 0)
            {
                if(UseSoftDelete)
                {
                    foreach(QmsEmployee emp in RecordsToDelete)
                    {
                       QmsCore.Data.Context.Update(emp);
                       Logger.Log.Record(string.Format("Employee {0} {1},{2} set to inactive in database",emp.EmplId,emp.LastName,emp.FirstName));                    
                    }
                }
                else
                {
                    foreach(QmsEmployee emp in RecordsToDelete)
                    {
                        QmsCore.Data.Context.Remove(emp);
                        Logger.Log.Record(string.Format("Employee {0} {1},{2} deleted from database",emp.EmplId,emp.LastName,emp.FirstName));                    
                    }
                }
                QmsCore.Data.Context.SaveChanges();
                j++;
            }
            Logger.Log.Record(string.Format("{0} records to be deleted. {1} records deleted",i,j));

        }
        private void update()
        {
            int i = RecordsToUpdate.Count;
            int j = 0;
            if(i > 0)
            {
                foreach(QmsEmployee emp in RecordsToUpdate)
                {
                    QmsCore.Data.Context.Update(emp);
                    Logger.Log.Record(string.Format("Employee {0} {1},{2} updated in database",emp.EmplId,emp.LastName,emp.FirstName));                    
                }
                QmsCore.Data.Context.SaveChanges();
                j++;
            }
            Logger.Log.Record(string.Format("{0} records to be updated. {1} records updated",i,j));
            
        }

        private void insert()
        {
            int i = RecordsToInsert.Count;
            int j = 0;
            if(i > 0)
            {
                foreach(QmsEmployee emp in RecordsToInsert)
                {
                    QmsCore.Data.Context.Add(emp);
                    Logger.Log.Record(string.Format("Employee {0} {1},{2} added to database",emp.EmplId,emp.LastName,emp.FirstName));                    
                }
                QmsCore.Data.Context.SaveChanges();
                j++;
            }
            Logger.Log.Record(string.Format("{0} records to be inserted. {1} records inserted",i,j));

        }






    }//end class
}//end namespace