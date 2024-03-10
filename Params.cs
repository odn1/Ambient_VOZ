﻿using System;

namespace ReportUT_
{
    [Serializable]
    public class Params
    {
        public string Room { get; set; } = "Undefined";
        public string DSN { get; set; } = "Undefined";
        public string Report { get; set; } = "Undefined";
        public string Sample { get; set; } = "Undefined";
        public string Date_POV { get; set; } = "Undefined";
        public bool Date_POV_check { get; set; } =  false;
        public bool Room_check { get; set; } = false;

        public Params() { }

    }
}
