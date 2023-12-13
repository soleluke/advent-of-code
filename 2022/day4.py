#!/usr/bin/env python3

file = open("puzzle_input","r")
lines = file.readlines()
inp = [line.strip() for line in lines]

test_input = ['2-4,6-8','2-3,4-5','5-7,7-9','2-8,3-7','6-6,4-6','2-6,4-8']

def get_pairs(input):
  pair = input.split(',')
  splits = [elf.split('-') for elf in pair]
  return [[int(elf[0]),int(elf[1])] for elf in splits]


def pair_contained(first,second):
  result = first[1] >= second[1] and first[0] <= second[0]
  return result
def pair_overlap(first,second):
  result = first[1] >= second[1] and first[0] <= second[1]
  return result or pair_contained(first,second)

contained = []

pairs = [get_pairs(line) for line in inp]
print(pairs)
for pair in pairs:
  if pair_overlap(pair[0],pair[1]) or pair_overlap(pair[1],pair[0]):
    contained.append(pair)
print(len(contained))