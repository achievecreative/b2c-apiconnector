﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace b2c_ApiConnector.Models
{
    public class ResponseObject
    {
        public string Version { get; set; } = "1.0.0";

        public string Action { get; set; }

        public string Company { get; set; }
    }
}
