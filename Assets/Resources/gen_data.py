import json
import random
import time
from datetime import datetime, timezone

def generate_sensor_data(sensor_id):
    timestamp_utc = int(time.time())
    timestamp_ms = int((time.time() % 1) * 100000)

    return {
        "sensorId": sensor_id,
        "HR": {
            "rrData": [random.randint(300, 500)],
            "Movesense_series": "174630000192",
            "Pico_ID": "pico_{}".format(sensor_id),
            "Timestamp_UTC": timestamp_utc,
            "average": round(random.uniform(95, 110), 1)
        },
        "ECG": {
            "Samples": [random.randint(-65000, 65000) for _ in range(16)],
            "Movesense_series": "174630000192",
            "Pico_ID": "pico_{}".format(sensor_id),
            "Timestamp_UTC": timestamp_utc,
            "Timestamp_ms": timestamp_ms
        },
        "IMU9": {
            "Timestamp_UTC": timestamp_utc,
            "Movesense_series": "174630000192",
            "Pico_ID": "pico_{}".format(sensor_id),
            "Timestamp_ms": timestamp_ms + 50,
            "ArrayAcc": [{"x": round(random.uniform(7, 9), 3), "y": round(random.uniform(-6, -4), 3), "z": round(random.uniform(1, 2), 3)} for _ in range(4)],
            "ArrayGyro": [{"x": round(random.uniform(-2, 3), 2), "y": round(random.uniform(6, 10), 2), "z": round(random.uniform(5, 12), 2)} for _ in range(4)],
            "ArrayMagn": [{"x": round(random.uniform(42, 45), 2), "y": round(random.uniform(-29, -24), 2), "z": round(random.uniform(3, 6), 2)} for _ in range(4)]
        },
        "GNSS": {
            "GNSS_sensor_ID": "174630000192",
            "Date": datetime.now(timezone.utc).isoformat(),
            "Latitude": 60.223950 + random.uniform(0, 0.00001),
            "Longitude": 24.945630 + random.uniform(0, 0.00001)
        }
    }

def generate_player_data():
    return {
        "playerId": 12,
        "sensorSetId": 1,
        "sensors": [generate_sensor_data(i) for i in range(1, 4)]
    }

def save_json():
    player_data = generate_player_data()
    with open("Assets/Resources/sensor_data.json", "w") as f:
        json.dump(player_data, f, indent=2)

if __name__ == "__main__":
    save_json()