namespace WeightScale.Models.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public enum MeasurementStatus
    {
        OK = 0,
        TheWeightIsOverHighestAcceptable = 1,
        ProtocolPrinterFailure = 2,
        TheWeightScaleRamOverflow = 3,
        CalibrationMemoryFailure = 4,
        CalibrationMemoryIsUnderInitialization = 5,
        TheTransactionIsAlreadyFinished = 6,
        TheWeightIsUnderLowestAcceptable = 7,
        TheCarIsOutsideWeightScalePlatform = 8,
        TotalizerIsOverflow = 9
    }
}