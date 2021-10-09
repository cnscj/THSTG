#!/usr/bin/python
#coding:utf-8

import os
import re
import sys

# 确定绝对路径
def getPath(path, join):
	return os.path.abspath(os.path.join(path, join))

# 如果锁住了，尝试清一下锁
def svnCheckAndCleanup(folder):
	needCleanup = False

	cmd = "svn status {0}".format(folder)
	with os.popen(cmd) as f:
		for line in f.readlines():
			if line.find("L", 0, 8) != -1:
				needCleanup = True
				break
		f.close()

	if needCleanup:
		if os.system("svn cleanup {0}".format(folder)) != 0:
			print("[ERROR] svn cleanup failed.")
			return False

	return True

def svnCleanup(folder):
	tmpDir = getPath(folder, ".svn/tmp")
	if not os.path.exists(tmpDir):
		# 莫名的.svn:E000002错误，这里自己创建tmp目录
		os.system("mkdir " + tmpDir)
	if os.system("svn cleanup {0}".format(folder)) != 0:
		print("[ERROR] svn cleanup failed.")

# 一键更新，如果不存在，检出svn
def svnUpdateAnyway(dir, url):
	if os.path.exists(dir):
		svnRevertAndUpdate(dir)
	else:
		svnCheckoutIfNoExist(dir, url)
	if os.system("svn info {0}".format(dir)) != 0:
		sys.exit(1)

def svnCheckoutIfNoExist(dir, url):
	if not os.path.exists(dir):
		folder = os.path.basename(dir)
		if not os.path.exists(folder):
			os.makedirs(folder)

		os.system("echo " + dir + " does not exist, checkout now")
		cmd = "svn checkout {0} {1}".format(url, dir)
		os.system("echo " + cmd)
		os.system(cmd)

# 一键提交，有add有delete
def svnCommitAnyway(dir, msg, account = ""):
	svnAutoAdd(dir)
	svnAutoDelete(dir)
	if not svnCommit(dir, msg, account):
		sys.exit(1)

def svnRevertAndUpdate(dir):
	if os.path.exists(dir):
		if svnRevertDir(dir):
			if svnUpdate(dir):
				return
	sys.exit(1)

# manifest
def svnCommitWithOutManifest(dir, msg):
	svnAutoAddFilter(dir, "manifest")
	svnAutoDelete(dir)
	svnCommit(dir, msg)

# 一键还原，修改还原，新的移除
def svnRevertAnyway(dir):
	deleteUnusedFiles(dir)
	svnRevertDir(dir)
		
# svn状态
def svnStatus(dir):
	if os.path.exists(dir):
		cmd = "svn status " + dir
		os.system(cmd)

# svn更到指定版本
def svnUpdate(dir, toVersion = "HEAD"):
	if os.path.exists(dir) and svnCheckAndCleanup(dir):
		cmd = "svn up -r {0} {1}".format(toVersion, dir)
		return os.system(cmd) == 0
	else:
		return False

# svn回滚到最新
def svnRevertDir(dir):
	if os.path.exists(dir) and svnCheckAndCleanup(dir):
		cmd = "svn revert {0} --recursive".format(dir)
		return os.system(cmd) == 0
	else:
		return False

# svn提交
def svnCommit(dir, msg, account = ""):
	if os.path.exists(dir) and svnCheckAndCleanup(dir):
		svnDir = getPath(dir, ".svn")
		tmpDir = getPath(dir, ".svn/tmp")
		if os.path.exists(svnDir):
			if not os.path.exists(tmpDir):
				# 莫名的.svn:E000002错误，这里自己创建tmp目录
				os.system("mkdir " + tmpDir)
		cmd = 'svn commit -m "{0}" {1} {2}'.format(msg, dir, account)
		return os.system(cmd) == 0
	else:
		return False

def svnAddEmptyFolder(dir, msg):
	if not os.path.exists(dir):
		os.makedirs(dir)
		# os.system("mkdir " + dir)
		os.system("svn add --parents " + dir)
		os.system("svn commit -m {0} {1}".format(msg, dir))

def svnAutoAdd(dir):
	# svn status输出到文件，再读取文件拿到?信息去执行svn add
	if os.path.exists(dir):
		cmd = "svn status {0}".format(dir)
		with os.popen(cmd) as f:
			for line in f.readlines():
				if line[0] == "?":
					cmd = "svn add --parents " + line[8:len(line)-1]
					os.system("echo " + cmd)
					os.system(cmd)

# svn添加
def svnAutoAddFilter(dir, filter):
	# svn status输出到文件，再读取文件拿到?信息去执行svn add
	if os.path.exists(dir):
		cmd = "svn status {0}".format(dir)
		with os.popen(cmd) as f:
			for line in f.readlines():
				if line[0] == "?":
					file = line[8:len(line)-1]
					if not file.endswith(filter):
						cmd = "svn add --parents " + file
						os.system("echo cmd=" + cmd)
						os.system(cmd)

# svn删除missing文件
def svnAutoDelete(dir):
	# svn status输出到文件，再读取文件拿到!信息去执行svn delete
	if os.path.exists(dir):
		cmd = "svn status {0}".format(dir)
		with os.popen(cmd) as f:
			for line in f.readlines():
				if line[0] == "!":
					cmd = "svn delete " + line[8:len(line)-1]
					os.system("echo " + cmd)
					os.system(cmd)

# 清理svn未记录的文件
def deleteUnusedFiles(dir):
	# svn status输出到文件，再读取文件拿到?信息去执行delete
	if os.path.exists(dir):
		cmd = "svn status {0}".format(dir)
		with os.popen(cmd) as f:
			for line in f.readlines():
				if line[0] == "?":
					fileName = line[8:len(line)-1]
					if os.path.isfile(fileName):
						os.system("echo 移除非svn文件:" + fileName)
						os.remove(fileName)

# 获取当前版本号
def getCurrentVersion(dir):
	t = os.popen("svn info " + dir)
	var = t.read()
	list = var.splitlines()
	for str in list:
		if str.find('Revision:') >= 0:
			return re.findall("\d+", str)[0]

# # 获取版本间差异输出到文件
def logDiffToFile(dir, fromv, tov, savePath):
	os.system("svn diff {0} --summarize -r{1}:{2} > {3}".format(dir, fromv, tov, savePath))
