#!/bin/bash

echo $1

year=$1

[[ -d $year ]] || cp -r template $year
mkdir $year/inputs

ext=".cs"

for i in {01..25}; do
  day="Day$i"
  file="$day$ext"
  [[ -f $year/$file ]] || cp $year/Day.cs $year/$file
  sed -i "s/DayX/$day/" $year/$file
  inputs="day$i"
  mkdir $year/inputs/$inputs
  touch $year/inputs/$inputs/input
  touch $year/inputs/$inputs/test
done

mv $year/template.csproj $year/$year.csproj
rm $year/Day.cs
