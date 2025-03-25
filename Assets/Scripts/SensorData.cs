public class SensorData
{
    public float Timestamp;
    public float AccX, AccY, AccZ;
    public float GyroX, GyroY, GyroZ;
    public float MagnX, MagnY, MagnZ;

    public SensorData(float timestamp, float accX, float accY, float accZ, 
                      float gyroX, float gyroY, float gyroZ, 
                      float magnX, float magnY, float magnZ)
    {
        Timestamp = timestamp;
        AccX = accX;
        AccY = accY;
        AccZ = accZ;
        GyroX = gyroX;
        GyroY = gyroY;
        GyroZ = gyroZ;
        MagnX = magnX;
        MagnY = magnY;
        MagnZ = magnZ;
    }
    public override string ToString()
    {
        return $"Time: {Timestamp}s, Acc:({AccX}, {AccY}, {AccZ}), Gyro:({GyroX}, {GyroY}, {GyroZ}), Magn:({MagnX}, {MagnY}, {MagnZ})";
    }
}
