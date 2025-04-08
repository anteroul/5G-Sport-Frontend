import json
import random
import time
from datetime import datetime

JSON_FILE = "simulated_hockey_sensor_data_stream.json"

def generate_hr_data():
    return {
        "rrData": [random.randint(300, 800)],
        "Movesense_series": str(random.randint(100000000000, 999999999999)),
        "Timestamp_UTC": int(time.time()),
        "average": round(random.uniform(60, 180), 4)
    }

def generate_ecg_data():
    return {
        "Samples": [random.randint(-65000, 65000) for _ in range(16)],
        "Movesense_series": str(random.randint(100000000000, 999999999999)),
        "Timestamp_UTC": int(time.time()),
        "Timestamp_ms": random.randint(10000, 50000)
    }

def generate_vector3():
    return {
        "x": round(random.uniform(-10.0, 10.0), 3),
        "y": round(random.uniform(-10.0, 10.0), 3),
        "z": round(random.uniform(-10.0, 10.0), 3),
    }

def generate_imu9_data():
    return {
        "Timestamp_UTC": int(time.time()),
        "Timestamp_ms": random.randint(10000, 50000),
        "ArrayAcc": [generate_vector3() for _ in range(4)],
        "ArrayGyro": [generate_vector3() for _ in range(4)],
        "ArrayMagn": [generate_vector3() for _ in range(4)],
        "Movesense_series": str(random.randint(100000000000, 999999999999))
    }

def generate_gnss_data(player_id):
    return {
        "GNSS_sensor_ID": player_id,
        "Date": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
        "Latitude": round(random.uniform(59.0, 60.0), 6),
        "Longitude": round(random.uniform(24.0, 25.0), 6)
    }

def generate_player_data(player_id):
    return {
        "HR": generate_hr_data(),
        "ECG": generate_ecg_data(),
        "IMU9": generate_imu9_data(),
        "GNSS": generate_gnss_data(player_id)
    }

def load_existing_data():
    try:
        with open(JSON_FILE, "r") as f:
            return json.load(f)
    except FileNotFoundError:
        return {"players": {}}

def save_data(data):
    with open(JSON_FILE, "w") as f:
        json.dump(data, f, indent=4)

def simulate_stream(num_players=3, interval_seconds=2):
    print(f"Streaming simulated data every {interval_seconds}s. Press Ctrl+C to stop.")
    while True:
        data = load_existing_data()
        timestamp = datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%SZ")

        for i in range(1, num_players + 1):
            player_key = f"player_{i}"
            player_data = generate_player_data(player_key)

            if player_key not in data["players"]:
                data["players"][player_key] = {}

            # Append time-based entry
            data["players"][player_key][timestamp] = player_data

        save_data(data)
        print(f"Data written at {timestamp}")
        time.sleep(interval_seconds)

if __name__ == "__main__":
    simulate_stream(num_players=3, interval_seconds=5)
