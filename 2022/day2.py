#!/usr/bin/env python3

from enum import IntEnum

class Play(IntEnum):
  ROCK = 1
  PAPER = 2
  SCISSORS = 3
class Result(IntEnum):
  DRAW = 3
  WIN = 6
  LOSE = 0
wins = {Play.ROCK: Play.SCISSORS, Play.PAPER: Play.ROCK, Play.SCISSORS: Play.PAPER}
loses = {v:k for (k,v) in wins.items()}
print(loses)

def pointsPart1(them, me):
  if me == them:
    return int(me) + 3
  if them == wins[me]:
    return int(me) + 6
  else:
    return int(me)
def pointsPart2(them,me):
  points = int(me)
  if me == Result.WIN:
    return points + int(loses[them])
  elif me == Result.DRAW:
    return points + int(them)
  elif me == Result.LOSE:
    return points + int(wins[them])
  
def getThemPlay(input):
  if input == 'A':
    return Play.ROCK
  elif input == 'B':
    return Play.PAPER
  elif input == 'C':
    return Play.SCISSORS
def getMePlayPt1(input):
  if input == 'X':
    return Play.ROCK
  elif input == 'Y':
    return Play.PAPER
  elif input == 'Z':
    return Play.SCISSORS
def getMePlayPt2(input):
  if input == 'X':
    return Result.LOSE
  elif input == 'Y':
    return Result.DRAW
  elif input == 'Z':
    return Result.WIN



input = open("puzzle_input","r")
lines = input.readlines()
#lines = ['A Y', 'B X', 'C Z']


plays = []
for line in lines:
  play = line.split()
  plays.append([getThemPlay(play[0]),getMePlayPt2(play[1])])
print(plays)
playPoints = [pointsPart2(play[0],play[1]) for play in plays]
print(playPoints)
print(sum(playPoints))