import sys
import json
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestRegressor
from sklearn import metrics

f = open(sys.argv[1],)

data = json.load(f)

print("successful import of pandas, numpy and sklearn")

print(data)

f.close()