using System;

using QmsCore.Engine;


namespace QmsCore
{
    class Program
    {
        public static string Environment;
        static void Main(string[] args)
        {
//            securityDemo();
            Environment = getEnvironment(args);
            Core core = new Core();
            core.Execute();
        } //end main   

        static string getEnvironment(string[] args)
        {
            string retval = "dev";
            try
            {
                string testVal = args[0].Replace("-",string.Empty).ToLower();
                if(testVal == "dev" || testVal == "test" || testVal == "prod")
                {
                    retval = testVal;
                }
                else
                {
                    throw new Exception("Cannot determine the appropriate environment. Defaulting to DEV");
                }   
            }
            catch (System.Exception)
            {
                //throw x;
            }
           Console.WriteLine("Environment being used: " + retval);
            return retval;
        }

        static void securityDemo()
        {
            try
            {
                string ssn1 = "333224444"; //d56c4d35bf12e591ccd0863f8b9f3b7faa97dad0d068aa79424c3fd8bcfa0db99c2d8e6f0ec531357a0fc5b9c2bb5dbd59b44707a5758e2f69d7d118cad7ca7f
                string ssn2 = "012345678";
                string hashedSSN1 = QmsCore.Engine.Security.GetHashedValue(ssn1);
                string hashedSSN2 = QmsCore.Engine.Security.GetHashedValue(ssn2);

                if( QmsCore.Engine.Security.VerifyHashedValue(ssn1,hashedSSN1))
                {
                    Console.WriteLine(string.Format("The hash of {0} matches {1}",ssn1,hashedSSN1));
                }

                if( QmsCore.Engine.Security.VerifyHashedValue(ssn2,hashedSSN1))
                {
                    Console.WriteLine(string.Format("The hash of {0} matches {1}",ssn2,hashedSSN1));
                }
                else
                {
                     Console.WriteLine(string.Format("The hash of {0} does not match {1}",ssn2,hashedSSN1));
                }

            }
            catch (System.Exception x)
            {
                Console.WriteLine(x.ToString());
            }            
        }
    }//end class
}//end namespace
