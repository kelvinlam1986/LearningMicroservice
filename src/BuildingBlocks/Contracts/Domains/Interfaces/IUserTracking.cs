﻿namespace Contracts.Domains.Interfaces
{
    public interface IUserTracking
    {
        string CreatedBy { get; set; }
        string LastedModifiedBy { get; set; }
    }
}
