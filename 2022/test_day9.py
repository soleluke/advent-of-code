#!/usr/bin/env python3
from day import calculate_move

import unittest

class TestMoves(unittest.TestCase):
  def test_on(self):
    self.assertEqual(calculate_move('U',(0,0),(0,0)),(0,0))
  # up
  def test_up_above(self):
    self.assertEqual(calculate_move('U',(0,2),(0,0)),(0,1))
  def test_up_below(self):
    self.assertEqual(calculate_move('U',(0,0),(0,2)),(0,1))
  def test_up_left(self):
    self.assertEqual(calculate_move('U',(0,1),(1,0)),(1,0))
  def test_up_right(self):
    self.assertEqual(calculate_move('U',(2,1),(1,0)),(1,0))
  def test_up_up_left(self):
    self.assertEqual(calculate_move('U',(0,2),(1,0)),(0,1))
  def test_up_up_right(self):
    self.assertEqual(calculate_move('U',(2,2),(1,0)),(2,1))
  # down
  def test_down_above(self):
    self.assertEqual(calculate_move('D',(0,0),(0,0)),(0,0))
  def test_down_below(self):
    self.assertEqual(calculate_move('D',(0,0),(0,2)),(0,1))
  def test_down_left(self):
    self.assertEqual(calculate_move('D',(0,0),(1,1)),(1,1))
  def test_down_right(self):
    self.assertEqual(calculate_move('D',(2,1),(1,0)),(1,0))
  def test_down_down_right(self):
    self.assertEqual(calculate_move('D',(2,0),(1,2)),(2,1))
  def test_down_down_left(self):
    self.assertEqual(calculate_move('D',(0,0),(1,2)),(0,1))
  # left
  def test_left_above(self):
    self.assertEqual(calculate_move('L',(0,1),(1,0)),(1,0))
  def test_left_below(self):
    self.assertEqual(calculate_move('L',(0,1),(1,1)),(1,1))
  def test_left_left(self):
    self.assertEqual(calculate_move('L',(0,0),(0,2)),(0,1))
  def test_left_right(self):
    self.assertEqual(calculate_move('L',(1,0),(1,0)),(1,0))
  def test_left_down_left(self):
    self.assertEqual(calculate_move('L',(0,0),(1,1)),(1,1))
  def test_left_up_left(self):
    self.assertEqual(calculate_move('L',(0,1),(1,0)),(1,0))
  # right
  def test_right_above(self):
    self.assertEqual(calculate_move('R',(1,1),(1,0)),(1,0))
  def test_right_below(self):
    self.assertEqual(calculate_move('R',(1,0),(1,1)),(1,1))
  def test_right_left(self):
    self.assertEqual(calculate_move('R',(0,0),(0,1)),(0,1))
  def test_right_right(self):
    self.assertEqual(calculate_move('R',(2,0),(0,0)),(1,0))
  def test_right_down_right(self):
    self.assertEqual(calculate_move('R',(4,3),(2,4)),(3,3))
  def test_right_up_right(self):
    self.assertEqual(calculate_move('R',(1,1),(0,0)),(0,0))

  


if __name__ == "__main__":
    unittest.main()