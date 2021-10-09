#!/usr/bin/python
#coding:utf-8

import os

def getAbsPath(path, join):
	return os.path.abspath(os.path.join(path, join))