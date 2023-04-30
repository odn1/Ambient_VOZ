using AmbientService.src.models;

namespace AmbientService.src.services
{
    interface IConnector
    {
        Sensor[] GetAllSensors();

        MeasurementData GetMeasurementDataBySensorId(int id);

        int GetLastMeasurementDataIdBySensorId(int id);
 
        void OpenConnection();

        void CloseConnection();
        MeasurementData_DAY Get_DAY_MeasurementDataIdBySensorId(int id, string Time1);
    }
}
