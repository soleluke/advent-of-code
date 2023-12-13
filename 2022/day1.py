#!/usr/bin/env python3
import re

input = open("puzzle_input","r")
lines = input.readlines()
print(lines)

elves = []

current_items=[]
for line in lines:
  val = re.sub(r'[^0-9]','',line)
  if val == '':
    elves.append(current_items)
    current_items = []
  else:
    current_items.append(int(val))

calories = [sum(elf) for elf in elves]
print(calories)
print(sum(sorted(calories)[-3:]))