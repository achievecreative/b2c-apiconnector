﻿namespace b2c_ApiConnector;

public class PasswordComplexityRequest
{
    public string Password { get; set; }

    public string[] Sources { get; set; }
}
