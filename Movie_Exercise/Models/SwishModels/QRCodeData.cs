﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Exercise.Models.SwishModels
{
    public class QRCodeData
    {
        public string token { get; set; }
        public string format { get; set; }
        public int size { get; set; }
        public int border { get; set; }
        public bool transparent { get; set; }
    }
}
