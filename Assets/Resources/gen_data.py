import time
import random

csv_file = "sensor_data.csv"

def format_timestamp(timestamp):
    minutes = int(timestamp // 60)
    seconds = int(timestamp % 60)
    milliseconds = int((timestamp * 1000) % 1000)  # Get milliseconds from the timestamp
    return f"{minutes:02}:{seconds:02}:{milliseconds:03}"

while True:
    with open(csv_file, "r") as file:
        lines = file.readlines()

    new_lines = [lines[0]]  # Ignore header
    for line in lines[1:]:
        values = line.strip().split(",")
        if len(values) >= 10:
            values[0] = format_timestamp(time.time())           # Get timestamp from system time
            values[1] = str(round(random.uniform(-5, 5), 3))    # Modify AccX
            values[2] = str(round(random.uniform(-5, 5), 3))    # Modify AccY
            values[3] = str(round(random.uniform(5, 15), 3))    # Modify AccZ
            values[4] = str(round(random.uniform(-5, 5), 3))    # Modify GyroX
            values[5] = str(round(random.uniform(-5, 5), 3))    # Modify GyroY
            values[6] = str(round(random.uniform(-5, 5), 3))    # Modify GyroZ
            values[7] = str(round(random.uniform(-5, 5), 3))    # Modify MagnX
            values[8] = str(round(random.uniform(-5, 5), 3))    # Modify MagnY
            values[9] = str(round(random.uniform(-5, 5), 3))    # Modify MagnZ
        new_lines.append(",".join(values) + "\n")

    with open(csv_file, "w") as file:
        file.writelines(new_lines)

    print("CSV updated")
    time.sleep(0.1)  # Modify every 100ms
