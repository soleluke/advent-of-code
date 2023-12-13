#!/usr/bin/env python3
import re
import json

def add_visit(v,knot):
  x = knot[0]
  y = knot[1]
  if x in v.keys():
    if y in v[x].keys():
      v[x][y] = v[x][y] + 1
    else:
      v[x][y] = 1
  else:
    v[x] = {y:1}
  return v
def move_head(direction,knot):
  h_x = knot[0]
  h_y = knot[1]
  if direction == 'R':
    h_x += 1
  if direction == 'L':
    h_x -= 1
  if direction == 'D':
    h_y -= 1
  if direction == 'U':
    h_y += 1
  return h_x,h_y
def calculate_move(direction,head,knot):
  h_x = head[0] # 4 
  h_y = head[1] # 3
  t_x = knot[0] # 2
  t_y = knot[1] # 4
  if abs(h_x - t_x) == 2: # 4 - 2 = (2)
    t_x = t_x + int((h_x - t_x)/2) # 2 + (4-2)/2 = 3
    if abs(h_y - t_y) == 2:
      t_y = t_y + int((h_y - t_y)/2)
    elif abs(h_y - t_y) == 1:
      t_y = h_y
  if abs(h_y - t_y) == 2: # (2)
    t_y = t_y + int((h_y - t_y)/2) # t_y = 0 + 2/2 --- 1
    if abs(h_x - t_x) == 2: # 1 > -1
      t_x = t_x + int((h_x - t_x)/2)
    elif abs(h_x - t_x) == 1:
      t_x = h_x
  return t_x,t_y


if __name__ == "__main__":


  file = open("puzzle_input.txt","r")
  lines = [line.strip() for line in file.readlines()]

  movements = []

  for line in lines:
    matches = re.search(r'([A-Z])\s+(\d+)', line)
    movements.append([matches.group(1),int(matches.group(2))])

  visits = {}

  knots = []
  for i in range(10):
    knots.append((0,0))

  for move in movements:
    direction = move[0]
    dist = move[1]
    print(direction)
    for d in range(dist):
      for i in range(len(knots)):
        if i == 0:
          knots[i] = move_head(direction,knots[i])
        else:
          knots[i] = calculate_move(direction,knots[i-1],knots[i])
      print(knots)
      visits = add_visit(visits, knots[-1])





  positions = [ len([v for v in visit.values() if v > 0]) for visit in visits.values()]

  print(positions)
  print(sum(positions))


  print(calculate_move('D',(1,0),(1,1)))