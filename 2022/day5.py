#!/usr/bin/env python3
import re
file = open("puzzle_input.txt","r")
lines = file.readlines()

imatch = r"move (\d+) from (\d+) to (\d+)"
inp = [line.rstrip('\n') for line in lines]

gridlines = []

while not re.match(imatch,inp[0]):
  gridlines.append(inp[0])
  inp = inp[1:]

cols = len(gridlines[-2].replace(' ',''))

gridlines = gridlines[:-2]

grid_re = r'( {3}|\[\S\]) {0,1}'

rows = [ [item.strip().replace('[','').replace(']','') for item in row] for row in [re.findall(grid_re,gridline) for gridline in gridlines]]

stacks = [[]] * cols

for row in rows:
  for i,item in enumerate(row):
    if item is not '':
      stacks[i] = [item] + stacks[i]
print(stacks)
  
    
  

#
#stacks = [['Z','N'],
#['M','C','D'],
#['P']]

#instructions = [
#'move 1 from 2 to 1',
#'move 3 from 1 to 3',
#'move 2 from 2 to 1',
#'move 1 from 1 to 2'
#]
#
#

for instruction in inp:
  nums = re.search(imatch, instruction)
  num_elements = int(nums.group(1))
  from_stack = int(nums.group(2)) - 1
  to_stack = int(nums.group(3)) - 1

  if not stacks[from_stack] or len(stacks[from_stack]) < num_elements:
    continue
  # remove the elements from the from stack
  # we use the slicing syntax to remove a slice of the specified length from the beginning of the stack
  # and store it in a variable so that we can add it to the to stack later
  moved_elements = stacks[from_stack][-num_elements:]
  stacks[from_stack] = stacks[from_stack][:-num_elements]
  stacks[to_stack] = stacks[to_stack] + moved_elements

print(''.join([stack[-1] for stack in stacks]))