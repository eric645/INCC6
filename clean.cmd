@for /r /d %%p in (bin) do @rd /s/q %%p 2>NUL
@for /r /d %%p in (obj) do @rd /s/q %%p 2>NUL
@for /r /d %%p in (x86) do @rd /s/q %%p 2>NUL
cd deployment
del *.pdb
del INCC6.exe
del INCCCmd.exe
del NCCCtrl.dll
del Defs.dll
del RepDB.dll