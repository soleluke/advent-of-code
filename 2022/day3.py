#!/usr/bin/env python3

input = open("puzzle_input","r")
lines = input.readlines()


test_input=['vJrwpWtwJgWrhcsFMMfFFhFp',
'jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL',
'PmmdzqPrVvPwwTWBwg',
'wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn',
'ttgJtRGJQctTZtZT',
'CrZsJsPPZsGzwwsLwLmpwMDw']

def split_rucksack(line):
  comp_size = int(len(line) / 2)
  return [line[0:comp_size], line[comp_size:]]
def item_priority(item):
  asci = ord(item)
  if asci >=97:
    return asci - 96
  else:
    return asci - 38
def get_compartments(input):
  compartments = []
  for line in input:
    compartments.append(split_rucksack(line.strip()))
  return compartments
  
def find_matching_item(sack):
  for item in sack[0]:
    if item in sack[1]:
      return item

def get_misplaced_item(input):
  compartments = get_compartments(input)

  items = []
  for sack in compartments:
    items.append(find_matching_item(sack))

  priorities = [ item_priority(i) for i in items]
  return sum(priorities)

def group_sacks(sacks):
  groups = []
  while len(sacks) > 0:
    groups.append(sacks[:3])
    sacks = sacks[3:]
  return groups

def find_matching_group_item(group):
  print(group)
  for item in group[0]:
    if item in group[1] and item in group[2]:
      return item

def badges(input):
  groups = group_sacks(input)
  items = [find_matching_group_item(g) for g in groups]
  priorities = [item_priority(i) for i in items]
  return sum(priorities)
  


print(badges(lines))