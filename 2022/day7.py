#!/usr/bin/env python3
import re
import json
file = open("puzzle_input.txt","r")
lines = [line.strip() for line in file.readlines()]
lines.reverse()

folders = {}

cd_re = r'\$\s+cd\s+(.+)'
ls_re = r'\$\s+ls'
dir_re = r'dir\s(.+)'
file_re = r'(\d+)\s(.+)'

current_folder = None

while len(lines) > 0:
  line = lines.pop()
  cd_match = re.search(cd_re,line)
  if cd_match is not None:
    target = cd_match.group(1)
    if target == '..':
      current_folder = current_folder['parent']
    else:
      if current_folder is None:
        folders[target] = {'parent':folders}
        current_folder = folders[target]
      else:
        if target not in current_folder.keys():
          current_folder[target] = {'parent': current_folder}
        current_folder = current_folder[target]
    continue
  ls_match = re.search(ls_re,line)
  if ls_match is not None:
    while True and len(lines) > 0:
      line = lines.pop()
      if line.startswith('$'): # is a command
        lines.append(line)
        break
      dir_match = re.search(dir_re, line)
      if dir_match is not None:
        d = dir_match.group(1)
        if d not in current_folder.keys():
          current_folder[d] = {'parent': current_folder}
        continue
      file_match = re.search(file_re,line)
      if file_match is not None:
        size = file_match.group(1)
        f = file_match.group(2)
        current_folder[f] = {'size':size}
        continue
    continue

current_folder = folders['/']

big_sizes = []



def get_directory_size(dir):
  size = 0
  for item in dir.items():
    if item[0] == 'parent' or item[0] == 'size':
      continue
    if type(item[1]) is dict:
      if 'parent' in item[1].keys():
        size = size + get_directory_size(item[1])
      elif 'size' in item[1].keys():
        size = size + int(item[1]['size'])
  dir['size'] = size
  if size <= 100000:
    big_sizes.append(size)
  return size

total_size = get_directory_size(folders)

folders['/']['size'] = total_size

system_size = 70000000
free_space_needed = 30000000

free_space = system_size - total_size

to_delete = free_space_needed - free_space


def check_dir(dir,delete_size):
  for item in dir.items():
    if item[0] == 'parent' or item[0] == 'size':
      continue
    if type(item[1]) is dict:
      if 'parent' in item[1].keys():
        delete_size = check_dir(item[1],delete_size)
        size = int(item[1]['size'])
        if size >= to_delete and size <= delete_size:
          delete_size = size
  return delete_size


delete_size = check_dir(folders,total_size)

print(delete_size)