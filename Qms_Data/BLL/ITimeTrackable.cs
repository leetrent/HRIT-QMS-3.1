using System;
using System.Collections.Generic;

namespace QmsCore.Model
{
    public interface ITimeTrackable
    {
        int DaysSinceCreated {get;}
        int? DaysSinceAssigned {get;}

    }
}