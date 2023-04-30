using AmbientService.src.models;
using AmbientService.src.services;
using AmbientService.src.services.impls;
using AmbientService.src.utils;
using System;
using System.Collections.Generic;

namespace AmbientService.src.controllers.impls
{
    class SensorControllerImpl : ISensorController
    {
        #region Singlton
        private static ISensorController instanse;

        public static ISensorController GetInstanse()
        {
            return instanse is null ? instanse = new SensorControllerImpl() : instanse;
        }
        #endregion

        private List<Sensor> _sensors;
        private IConnector _connector;
        
        private SensorControllerImpl()
        {
            _sensors = new List<Sensor>();
            _connector = OdbcConnector.GetInstanse();
            _connector.OpenConnection();
        }

        public void UpdateSensorsDAY()
        {
            Sensor[] sensorArray = _connector.GetAllSensors();

            foreach (Sensor sensor in sensorArray)
            {
                int positionInList = GetSensorPosition(sensor);

                if (positionInList < 0)
                {
                    positionInList = _sensors.Count;
                    _sensors.Add(sensor);
                }

                //   _sensors[positionInList].Measurement = _connector.GetMeasurementDataBySensorId(sensor.Id);
                _sensors[positionInList].Measurement_DAY = _connector.Get_DAY_MeasurementDataIdBySensorId(sensor.Id, "14.04.2022 08:59:59");

            }
        }

        public void UpdateSensors()
        {
            Sensor[] sensorArray = _connector.GetAllSensors();

            foreach (Sensor sensor in sensorArray)
            {
                int positionInList = GetSensorPosition(sensor);

                if (positionInList < 0)
                {
                    positionInList = _sensors.Count;
                    _sensors.Add(sensor);
                }

                _sensors[positionInList].Measurement = _connector.GetMeasurementDataBySensorId(sensor.Id);

                if (_sensors[positionInList].Measurement.Time > _sensors[positionInList].LastDataTime)
                {
                    _sensors[positionInList].LastDataTime = _sensors[positionInList].Measurement.Time;

                    if (_sensors[positionInList].IsNotContact)
                    {
                        _sensors[positionInList].IsNotContact = false;
                        EventCollection.CallonContact(_sensors[positionInList]);
                    }
                }
                else
                {
                    if (IsTimeOut(_sensors[positionInList].LastDataTime,
                        Convert.ToInt16(IniReader.Read("NotContact", "Time"))))
                    {
                        _sensors[positionInList].IsNotContact = true;
                        _sensors[positionInList].LastDataTime = DateTime.Now;
                        EventCollection.CallNotContact(_sensors[positionInList]);
                    }
                }

                EnumOutState temperatureState = TemperatureChangeBeyondState(_sensors[positionInList]);
                EnumOutState humidityState = HumidityChangeBeyondState(_sensors[positionInList]);
                EnumOutState pressureState = PressureChangeBeyondState(_sensors[positionInList]);


                if (temperatureState != EnumOutState.None 
                    || humidityState != EnumOutState.None 
                    || pressureState != EnumOutState.None
                    && IsTimeOut(_sensors[positionInList].LastSendEmailData, Convert.ToInt16(IniReader.Read("DataTime", "Time"))))
                {
                    if (temperatureState == EnumOutState.Out || humidityState == EnumOutState.Out || pressureState == EnumOutState.Out)
                    {
                        _sensors[positionInList].LastSendEmailData = DateTime.Now;
                        EventCollection.CallOutBeyond(_sensors[positionInList]);
                    }
                    if (temperatureState == EnumOutState.Normal || humidityState == EnumOutState.Normal || pressureState == EnumOutState.Normal)
                    {
                        EventCollection.CallInBeyond(_sensors[positionInList]);
                    }
                }

                if (IsLowBattery(_sensors[positionInList].Measurement) 
                    && IsTimeOut(_sensors[positionInList].LastSendEmailLowBattary, Convert.ToInt16(IniReader.Read("LowBattery", "Time"))))
                {
                    _sensors[positionInList].LastSendEmailLowBattary = DateTime.Now;
                    EventCollection.CallLowBattery(_sensors[positionInList]);
                }
            }
        }

        private int GetSensorPosition(Sensor sensor)
        {
            return _sensors.FindIndex(x => x.Id == sensor.Id);
        }

        private bool IsLowBattery(MeasurementData value)
        {
            return value.BatteryLevel < 3.5;
        }

        private EnumOutState TemperatureChangeBeyondState(Sensor sensor)
        {
            if (IsOut(sensor.tempOut, 
                sensor.Measurement.Temperature, 
                sensor.Measurement.MinTemperature, 
                sensor.Measurement.MaxTemperature))
            {
                sensor.tempOut = sensor.Measurement.Temperature > sensor.Measurement.MaxTemperature ? OutBoundEnum.Max : OutBoundEnum.Min;

                return EnumOutState.Out;
            }
            if (InNormal(sensor.tempOut, 
                sensor.Measurement.Temperature, 
                sensor.Measurement.MinTemperature, 
                sensor.Measurement.MaxTemperature))
            {
                return EnumOutState.Normal;
            }

            return EnumOutState.None;
        }

        private EnumOutState HumidityChangeBeyondState(Sensor sensor)
        {
            if (IsOut(sensor.humOut,
                sensor.Measurement.Humidity,
                sensor.Measurement.MinHumidity,
                sensor.Measurement.MaxHumidity))
            {
                sensor.humOut = sensor.Measurement.Humidity > sensor.Measurement.MaxHumidity ? OutBoundEnum.Max : OutBoundEnum.Min;

                return EnumOutState.Out;
            }
            if (InNormal(sensor.humOut,
                sensor.Measurement.Humidity,
                sensor.Measurement.MinHumidity,
                sensor.Measurement.MaxHumidity))
            {
                return EnumOutState.Normal;
            }

            return EnumOutState.None;
        }

        private EnumOutState PressureChangeBeyondState(Sensor sensor)
        {
            if (IsOut(sensor.presOut,
                sensor.Measurement.Pressure,
                sensor.Measurement.MinPressure,
                sensor.Measurement.MaxPressure))
            {
                sensor.presOut = sensor.Measurement.Pressure > sensor.Measurement.MaxPressure ? OutBoundEnum.Max : OutBoundEnum.Min;

                return EnumOutState.Out;
            }
            if (InNormal(sensor.presOut,
                sensor.Measurement.Pressure,
                sensor.Measurement.MinPressure,
                sensor.Measurement.MaxPressure))
            {
                return EnumOutState.Normal;
            }

            return EnumOutState.None;
        }

        private bool IsOut(OutBoundEnum type, float value, float min, float max)
        {
            return type == OutBoundEnum.None && (value > max || value < min);
        }

        private bool InNormal(OutBoundEnum type, float value, float min, float max)
        {
            return (type == OutBoundEnum.Min && value >= min) || (type == OutBoundEnum.Max && value <= max);
        }

        private bool IsTimeOut(DateTime lastSend, int interval)
        {
            var tmp = lastSend.AddMinutes(interval);
            return lastSend == DateTime.MinValue ? true : tmp <= DateTime.Now;
        }
    }
}
