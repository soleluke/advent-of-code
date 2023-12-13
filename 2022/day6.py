#!/usr/bin/env python3
file = open("puzzle_input.txt","r")
lines = [line.strip() for line in file.readlines()]
print(lines)

for line in lines:
  for i,char in enumerate(line):
    if i >= 13:
      count = True
      section = line[i-13:i+1]
      for c in section:
        if section.count(c) > 1:
          count = False
      if count:
        print(section)
        print(i+1)
        break
