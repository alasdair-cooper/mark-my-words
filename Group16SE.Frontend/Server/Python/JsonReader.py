import sys
import json
import pandas as pd
import numpy as np

f = open(sys.argv[1],)

data = json.load(f)

print(data)

f.close()