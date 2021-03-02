import sys
import json

f = open(sys.argv[1],)

data = json.load(f)

print(data)

f.close()