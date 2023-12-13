#!/usr/bin/env python3
import re
import json
file = open("puzzle_input.txt","r")
lines = [line.strip() for line in file.readlines()]

def outer(trees):
  visible = 0
  for i,line in enumerate(trees):
    for j,tree in enumerate(line):
      if i == 0 or i == len(trees)-1 or j == 0 or j == len(line)-1:
        visible+=1
  return visible

def visible(trees):
  visible = 0
  for i,line in enumerate(trees):
    for j,tree in enumerate(line):
      if not( i == 0 or i == len(trees)-1 or j == 0 or j == len(line)-1):
        left_vis = True
        right_vis = True
        top_vis = True
        bottom_vis = True
        column = [l[j] for l in trees]
        for redo,test in enumerate(line):
          if test >= tree and redo < j:
            left_vis = False
          if test >= tree and redo > j:
            right_vis = False
        for c,test in enumerate(column):
          if test >= tree and c < i:
            top_vis = False
          if test >= tree and c > i:
            bottom_vis = False
        if left_vis or right_vis or top_vis or bottom_vis:
          visible += 1
  return visible

def distance(trees):
  dists = []
  for i,line in enumerate(trees):
    dist_line = []
    for j, tree in enumerate(line):
      top_dist = 0
      bot_dist = 0
      left_dist = 0
      right_dist = 0
      if i > 0:
        column = [l[j] for k,l in enumerate(trees) if k < i]
        column.reverse()
        for t in column:
          top_dist +=1
          if t >= tree:
            break
      if i < len(trees) - 1:
        column = [l[j] for k,l in enumerate(trees) if k > i]
        for t in column:
          bot_dist +=1
          if t >= tree:
            break
      if j > 0:
        row = [(k,l) for k,l in enumerate(line) if k < j]
        row.reverse()
        for _,t in row:
          left_dist += 1
          if t >= tree:
            break
      if j < len(line) - 1:
        row = [(k,l) for k,l in enumerate(line) if k > j]
        for _,t in row:
          right_dist += 1
          if t >= tree:
            break
      dist_line.append(top_dist*bot_dist*left_dist*right_dist)



    dists.append(dist_line)
  return dists


trees = [ [int(c) for c in line] for line in lines]

print(outer(trees) + visible(trees))
print(max([max(l) for l in distance(trees)]))
