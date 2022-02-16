import os
from os import path
from pathlib import Path
import subprocess
from glob import glob
import sys

home_dir = path.dirname(__file__)

commands = []

if os.getcwd() is not home_dir:
    os.chdir(home_dir)

os.chdir(Path(os.getcwd()).parent.absolute())

if Path("GL_EditorFramework").exists() is not True and Path("GL_EditorFramework").is_dir() is not True:
    subprocess.call("git clone https://github.com/jupahe64/GL_EditorFramework.git", shell=True)

os.chdir("GL_EditorFramework")

subprocess.call("git pull", shell=True)

subprocess.call("nuget restore", shell=True)

os.chdir(home_dir)

subprocess.call("nuget restore", shell=True)

files = glob("*.sln")

for file in files:
    commands.append(f'msbuild "{path.abspath(file)}" -p:Configuration="Release Spotlight"')
    commands.append(f'msbuild "{path.abspath(file)}" -p:Configuration="Release Moonlight" -p:Platform=x64')

for command in commands:
    subprocess.call(command, shell=True)

os.chdir("SpotLight\\bin\\Moonlight_RELEASE")

subprocess.call(f'7z a "{home_dir}\\Moonlight.zip" *.* -r')

os.chdir(f"{Path(os.getcwd()).parent.parent.absolute()}\\Spotlight_RELEASE")

subprocess.call(f'7z a "{home_dir}\\Spotlight.zip" *.* -r')

os.chdir(home_dir)

sys.exit(0)