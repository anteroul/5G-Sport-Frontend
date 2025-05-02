[System.Serializable]
public class Root
{
    public int playerId;
    public int sensorSetId;
    public Sensor[] sensors;
}

[System.Serializable]
public class Sensor
{
    public int sensorId;
    public HR HR;
    public ECG ECG;
    public IMU9 IMU9;
    public GNSS GNSS;
}

[System.Serializable]
public class HR
{
    public int[] rrData;
    public string Movesense_series;
    public string Pico_ID;
    public long Timestamp_UTC;
    public float average;
}

[System.Serializable]
public class ECG
{
    public int[] Samples;
    public string Movesense_series;
    public string Pico_ID;
    public long Timestamp_UTC;
    public int Timestamp_ms;
}

[System.Serializable]
public class IMU9
{
    public long Timestamp_UTC;
    public string Movesense_series;
    public string Pico_ID;
    public int Timestamp_ms;
    public Vector3Array[] ArrayAcc;
    public Vector3Array[] ArrayGyro;
    public Vector3Array[] ArrayMagn;
}

[System.Serializable]
public class GNSS
{
    public string GNSS_sensor_ID;
    public string Date;
    public double Latitude;
    public double Longitude;
}

[System.Serializable]
public class Vector3Array
{
    public float x;
    public float y;
    public float z;
}