import json
import random
import time
from datetime import datetime, timezone

DATA_FILE = 'sensor_packets.json'
NUM_PLAYERS = 3
UPDATE_INTERVAL = 0.1  # seconds

def generate_ecg_samples():
    return [random.randint(-65000, 65000) for _ in range(16)]

def generate_acc_samples():
    return [
        {"x": round(random.uniform(7, 9), 3),
         "y": round(random.uniform(-6, -4), 3),
         "z": round(random.uniform(1, 2), 3)}
        for _ in range(4)
    ]

def generate_gyro_samples():
    return [
        {"x": round(random.uniform(-2, 3), 2),
         "y": round(random.uniform(6, 10), 2),
         "z": round(random.uniform(5, 12), 2)}
        for _ in range(4)
    ]

def generate_magn_samples():
    return [
        {"x": round(random.uniform(42, 45), 2),
         "y": round(random.uniform(-29, -24), 2),
         "z": round(random.uniform(3, 6), 2)}
        for _ in range(4)
    ]

def generate_sensor_packet(player_id):
    now_unix = int(time.time())
    timestamp_ms = int((time.time() * 1000) % 100000)
    movesense_id = str(174630000190 + player_id)
    pico_id = f"pico_{player_id}"
    current_hr = round(random.uniform(95, 110), 1)

    return {
        "playerId": player_id,
        "sensorSetId": 1,
        "sensors": [{
            "sensorId": 1,
            "HR": {
                "rrData": [int(60000 / current_hr)],
                "Movesense_series": movesense_id,
                "Pico_ID": pico_id,
                "Timestamp_UTC": now_unix,
                "average": current_hr
            },
            "ECG": {
                "Samples": generate_ecg_samples(),
                "Movesense_series": movesense_id,
                "Pico_ID": pico_id,
                "Timestamp_UTC": now_unix,
                "Timestamp_ms": timestamp_ms
            },
            "IMU9": {
                "Timestamp_UTC": now_unix,
                "Movesense_series": movesense_id,
                "Pico_ID": pico_id,
                "Timestamp_ms": timestamp_ms + 50,
                "ArrayAcc": generate_acc_samples(),
                "ArrayGyro": generate_gyro_samples(),
                "ArrayMagn": generate_magn_samples()
            },
            "GNSS": {
                "GNSS_sensor_ID": movesense_id,
                "Date": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
                "Latitude": 60.223950 + random.uniform(0, 0.00001),
                "Longitude": 24.945630 + random.uniform(0, 0.00001)
            }
        }]
    }

if __name__ == "__main__":
    while True:
        try:
            packet_list = [generate_sensor_packet(pid) for pid in range(NUM_PLAYERS)]

            with open(DATA_FILE, 'w') as f:
                json.dump(packet_list, f, indent=2)

            print(f"[{datetime.now().isoformat()}] Sensor packets updated for {NUM_PLAYERS} players.")
            time.sleep(UPDATE_INTERVAL)
        except Exception as e:
            print(f"Error: {e}")
            time.sleep(UPDATE_INTERVAL)
