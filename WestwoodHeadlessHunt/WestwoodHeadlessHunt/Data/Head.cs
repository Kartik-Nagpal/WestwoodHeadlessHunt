﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestwoodHeadlessHunt.Data
{
    public class Head
    {
        public int id { get; set; }
        public String name { get; set; }
        public ImageData[] images { get; set; }
        public int totalImageCount { get; set; }
    }
}
