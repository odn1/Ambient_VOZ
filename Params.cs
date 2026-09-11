using System;

namespace ReportUT_
{
    [Serializable]
    public class Params
    {
        public string Room { get; set; } = "Undefined";
        public string DSN { get; set; } = "UnitessDB";
        public string Report { get; set; } = " ";
        public string Sample { get; set; } = "C:\\Program Files (x86)\\Ambient Viewer\\ReportGen\\карта_температуры_и_влажности_Юнитесс.docx";
        public string Date_POV { get; set; } = "Undefined";
        public bool Date_POV_check { get; set; } =  false;
        public bool Room_check { get; set; } = false;

        public Params() { }

    }
}
