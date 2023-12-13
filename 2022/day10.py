#!/usr/bin/env python3
import re
from enum import Enum

class Inst(Enum):
  NOOP = 0
  ADD_X = 1

def update_screen(cur_cycle,screen,X,cur_pixel):
  pixel = '.'
  if cur_pixel[1] <= X + 1 and cur_pixel[1] >= X - 1:
    pixel = '#'
  print(str(cur_cycle)+':'+str(cur_pixel[1]) + ' ' + str(cur_pixel[0]) + ' ' + str(X)+'-'+pixel)
  screen[cur_pixel[0]][cur_pixel[1]] = pixel


def print_screen(screen):
  for line in screen:
    print("".join(line))

def inc_cycle(cur_cycle,X,signals,screen,cur_pixel):
  if (cur_cycle - 20) % 40 == 0:
    signals.append(cur_cycle*X)
  update_screen(cur_cycle,screen,X,cur_pixel)
  cur_pixel[1] = cur_pixel[1] + 1
  if cur_pixel[1] > 39:
    cur_pixel[0] = cur_pixel[0] + 1
    cur_pixel[1] = 0
  if cur_pixel[0] > 5:
    cur_pixel[0] = 0
  
  
  return cur_cycle + 1
def do_instruction(X,cur_cycle,instr, p, signals,screen,cur_pixel):
  if instr == Inst.NOOP:
    return X, inc_cycle(cur_cycle,X,signals,screen,cur_pixel) 
  if instr == Inst.ADD_X:
    cur_cycle = inc_cycle(cur_cycle,X,signals,screen,cur_pixel)
    cur_cycle = inc_cycle(cur_cycle,X,signals,screen,cur_pixel)
    return X + int(p), cur_cycle
def get_instruction(line):
  if re.search(r'noop',line) is not None:
    return Inst.NOOP,None
  match = re.search(r'addx\s+(-*\d+)', line)
  if match is not None:
    return Inst.ADD_X,match.group(1)

if __name__ == "__main__":
  file = open("puzzle_input.txt","r")
  lines = [line.strip() for line in file.readlines()]

  X = 1
  cur_cycle = 1
  signals = []
  screen = []
  cur_pixel = [0,0]
  for i in range(6):
    line = []
    for g in range(40):
      line.append('.')
    screen.append(line)
  for line in lines:
    inst,p = get_instruction(line)
    X,cur_cycle = do_instruction(X,cur_cycle, inst,p, signals,screen,cur_pixel)
  
  print_screen(screen)
  print(sum(signals))
