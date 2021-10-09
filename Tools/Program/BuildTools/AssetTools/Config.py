#!/usr/bin/python
#coding:utf-8

import os
import sys

PROJECT_ID = "1"

UNITY_MAC_EXE_PATH = "/Applications/Unity/2018.4.11f1/Unity.app/Contents/MacOS/Unity"
UNITY_WIN_EXE_PATH = "C:/Unity/Unity.exe"

def getUnityExePath():
    if sys.platform == 'win32':
        return UNITY_WIN_EXE_PATH
    else:
        return UNITY_MAC_EXE_PATH