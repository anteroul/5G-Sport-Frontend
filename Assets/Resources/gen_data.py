import json
import random
import time
from datetime import datetime, timezone

DATA_FILE = 'sensor_data.json'
SENSOR_IDS = [1, 2, 3]
UPDATE_INTERVAL = 0.1  # seconds

def generate_sensor_data(sensor_id, player_id):
    timestamp_utc = int(time.time())
    timestamp_ms = int((time.time() % 1) * 100000)
    movesense_id = int(174630000190 + player_id)

    return {
        "HR": {
            "rrData": [random.randint(300, 500)],
            "Movesense_series": str(movesense_id),
            "Pico_ID": f"pico_{sensor_id}_player{player_id}",
            "Timestamp_UTC": timestamp_utc,
            "average": round(random.uniform(95, 110), 1)
        },
        "ECG": {
            "Samples": [random.randint(-65000, 65000) for _ in range(16)],
            "Movesense_series": str(movesense_id),
            "Pico_ID": f"pico_{sensor_id}_player{player_id}",
            "Timestamp_UTC": timestamp_utc,
            "Timestamp_ms": timestamp_ms
        },
        "IMU9": {
            "Timestamp_UTC": timestamp_utc,
            "Movesense_series": str(movesense_id),
            "Pico_ID": f"pico_{sensor_id}_player{player_id}",
            "Timestamp_ms": timestamp_ms + 50,
            "ArrayAcc": [{"x": round(random.uniform(7, 9), 3), "y": round(random.uniform(-6, -4), 3), "z": round(random.uniform(1, 2), 3)} for _ in range(4)],
            "ArrayGyro": [{"x": round(random.uniform(-2, 3), 2), "y": round(random.uniform(6, 10), 2), "z": round(random.uniform(5, 12), 2)} for _ in range(4)],
            "ArrayMagn": [{"x": round(random.uniform(42, 45), 2), "y": round(random.uniform(-29, -24), 2), "z": round(random.uniform(3, 6), 2)} for _ in range(4)]
        },
        "GNSS": {
            "GNSS_sensor_ID": str(movesense_id),
            "Date": datetime.now(timezone.utc).isoformat(),
            "Latitude": 60.223950 + random.uniform(0, 0.00001),
            "Longitude": 24.945630 + random.uniform(0, 0.00001)
        }
    }

def update_players(json_data, pid=0):
    for player in json_data:
        if not isinstance(player, dict) or 'playerId' not in player:
            continue

        if 'sensors' not in player or not isinstance(player['sensors'], list):
            player['sensors'] = []

        sensor_map = {s['sensorId']: s for s in player['sensors'] if isinstance(s, dict) and 'sensorId' in s}
        updated_sensors = []

        for sid in SENSOR_IDS:
            new_data = generate_sensor_data(sid, pid)

            if sid in sensor_map:
                sensor = sensor_map[sid]
                # Overwrite only sensor data fields
                sensor['HR'] = new_data['HR']
                sensor['ECG'] = new_data['ECG']
                sensor['IMU9'] = new_data['IMU9']
                sensor['GNSS'] = new_data['GNSS']
                updated_sensors.append(sensor)
            else:
                # New sensor with full data
                sensor = {"sensorId": sid}
                sensor.update(new_data)
                updated_sensors.append(sensor)

        player['sensors'] = updated_sensors
        pid += 1

    return json_data

if __name__ == "__main__":
    while True:
        try:
            with open(DATA_FILE, 'r') as f:
                json_data = json.load(f)

            updated_json = update_players(json_data)

            with open(DATA_FILE, 'w') as f:
                json.dump(updated_json, f, indent=2)

            print(f"[{datetime.now().isoformat()}] Sensor data updated for all players.")
            time.sleep(UPDATE_INTERVAL)
        except Exception as e:
            print(f"Error: {e}")
            time.sleep(UPDATE_INTERVAL)
