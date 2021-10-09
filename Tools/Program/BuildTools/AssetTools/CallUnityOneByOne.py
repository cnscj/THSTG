#!/usr/bin/python
#coding:utf-8

import sys
import time
import os

## 执行命令语句 func是函数名
##cmdEx = "-executeMethod GYEditor.BuildEngine." + func

def findErrors(inputFilePath):
    if not os.path.exists(inputFilePath):
        return None

    errors = []
    with open(inputFilePath, "rb") as f:
        for line in f.readlines():
            if line.find(": error CS") > -1 or line.find("Unhandled Exception:") > -1:
                errors.append(line)
        f.close()
    return errors

# 一个一个地调用Unity命令行（一个unity工程不能同时打开）
class CallUnityOneByOne:
	def __init__(self, projectDir, unityExePath, lockFileName = None):
		self.__projectDir = projectDir
		self.__unityExePath = unityExePath

		self.__lockFileName = projectDir + ".lock"
		if lockFileName:
			self.__lockFileName = lockFileName + ".lock"
		names = os.path.split(projectDir)
		logsDir = os.path.join(names[0], "Logs/")
		if not os.path.exists(logsDir):
			os.makedirs(logsDir)
		self.__logFileName = os.path.join(logsDir, names[1] + "_log.txt")

	def __del__(self):
		self.__unlock()

	def __isLock(self):
		return os.path.exists(self.__lockFileName)

	def __lock(self):
		if not self.__isLock():
			with open(self.__lockFileName, "w") as f :
				f.write("1")
				f.close()
	
	def __unlock(self):
		if self.__isLock():
			os.remove(self.__lockFileName)

	def getLogFilePath(self):
		return self.__logFileName

	# 等待后开始(上锁)
	def waitToStart(self, timeOutSecond, timeOutMsg):
		waitedSec = 0
		while self.__isLock():
			time.sleep(1)
			waitedSec = waitedSec + 1
			if waitedSec > timeOutSecond:
				os.system("echo {0}".format(timeOutMsg))
				sys.exit(1)
		self.__lock()
		return waitedSec

	# 结束（解锁）
	def finish(self):
		self.__unlock()

	def getUnityExe(self):
		return self.__unityExePath

	# 执行命令，成功返回True
	def doCommand(self, cmdEx):
		return self.doCommandOnPlatform(cmdEx, "")

	# 执行命令，成功返回True
	def doCommandOnPlatform(self, cmdEx, platformName):
		cmd = "{0} -projectPath {1} -batchmode -logFile {2} -quit "
		if platformName == "pc":
			platformName = "win"
		if platformName != "":
			cmd += "-buildTarget " + platformName + " "
		cmd += cmdEx

		exe = self.getUnityExe()
		cmd = cmd.format(exe, self.__projectDir, self.__logFileName) 

		startTime = time.time()
		os.system("echo 执行命令{0}".format(cmd))

		ret = os.system(cmd)

		endTime = time.time()
		os.system("echo 执行命令耗时：{0}秒".format(endTime - startTime))

		if ret == 0:
			errors = findErrors(self.__logFileName)
			if errors and len(errors) > 0:
				os.system("echo =====================================")
				os.system("echo 有编译错误，可打开{0}查看Unity日志。".format(self.__logFileName))
				for e in errors:
					os.system("echo {0}".format(e))
				os.system("echo =====================================")
				return False
			return True
		else:
			os.system("echo 命令：\n{0}\n".format(cmd))
			os.system("echo 处理失败，可打开{0}查看Unity日志。".format(self.__logFileName))
			return False

def create(projectDir, unityExePath, lockName = None):
	return CallUnityOneByOne(projectDir, unityExePath ,lockName)

