using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DaikinNet;

public class DeviceData
{
    public float TempIndoor { get; set; }
    public float TempOutdoor { get; set; }
    public float TempIndoorFarenheit => TempIndoor * 9 / 5 + 32;
    public float TempOutdoorFarenheit => TempOutdoor * 9 / 5 + 32;
    public float HumIndoor { get; set; }
    public float HumOutdoor { get; set; }
    public int AqOutdoorValue { get; set; }
    public AqLevel AqOutdoorLevel { get; set; }
    public AqLevel AqIndoorLevel { get; set; }
    public ThermostatStatus EquipmentStatus { get; set; }
    public ThermostatMode Mode { get; set; }

    [JsonProperty(PropertyName = "hspActive")]
    public float TargetHeatingTemperature { get; set; }

    public float TargetHeatingTemperatureFarhenheit => TargetHeatingTemperature * 9 / 5 + 32;

    [JsonProperty(PropertyName = "cspActive")]
    public float TargetCoolingTemperature { get; set; }

    public float TargetCoolingTemperatureeFarhenheit => TargetCoolingTemperature * 9 / 5 + 32;
    
    public bool FanCirculate { get; set; }
    
    public FanSpeed FanCirculateSpeed { get; set; }
    
    [JsonProperty(PropertyName = "ctControlAlgorithmCoolDemand")]
    public int CTControlAlgorithmCoolDemand { get; set; }
    
    [JsonProperty(PropertyName = "ctControlAlgorithmRawDemand")]
    public int CTControlAlgorithmRawDemand { get; set; }
    
    [JsonProperty(PropertyName = "ctCompressorCurrent")]
    public int CTCompressorCurrent { get; set; }

    public override string ToString()
    {
        return $" - EquipmentStatus: {EquipmentStatus}, Mode: {Mode}" +
               $"\n - TempIndoor: {TempIndoorFarenheit}, TempOutdoor: {TempOutdoorFarenheit}"
               + $"\n - HumIndoor: {HumIndoor}, HumOutdoor: {HumOutdoor}"
               + $"\n - AqOutdoorValue: {AqOutdoorValue}, AqOutdoorLevel: {AqOutdoorLevel}, AqIndoorLevel: {AqIndoorLevel}"
               + $"\n - TargetHeatingTemperature: {TargetHeatingTemperatureFarhenheit}, TargetCoolingTemperature: {TargetCoolingTemperatureeFarhenheit}"
               + $"\n - FanCirculate: {FanCirculate}, FanSpeed: {FanCirculateSpeed}";
    }

    public enum ThermostatMode
    {
        Off = 0,
        Heat = 1,
        Cool = 2,
        Auto = 3,
        AuxiliaryHeat = 4
    }

    public enum ThermostatStatus
    {
        UNKNOWN_0 = 0,
        Cooling = 1,
        Dehumidifying = 2,
        Heating = 3,
        Fan = 4,
        Idle = 5
    }

    public enum AqLevel
    {
        Good = 0,
        Moderate = 1,
        UnhealthySensitive = 2,
        Unhealthy = 3,
        VeryUnhealthy = 4,
        Hazardous = 5
    }    
    
    public enum FanSpeed
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }
}

