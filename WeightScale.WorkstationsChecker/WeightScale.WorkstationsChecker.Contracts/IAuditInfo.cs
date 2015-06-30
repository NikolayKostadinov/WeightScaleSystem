namespace WeightScale.WorkstationsChecker.Contracts
{
    using System;

    public interface IAuditInfo
    {
        DateTime CreatedOn { get; set; }

        bool PreserveCreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }

        string CreatedFrom { get; set; }

    }
}
