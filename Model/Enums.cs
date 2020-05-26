using System;
using System.Collections.Generic;
using System.Text;

namespace FileSpy.Model
{
    public class Enums
    {
        public enum FieldType
        {
            TextArea,
            TextBlock,
            Image,
            URI,
            Select
        }

        public enum WorkStatus 
        {
            Idle,
            Working,
            Canceled,
            Error,
            Done
        }
    }
}
