OptionExplicit
 
DimintReturn
Dimws
Setws=CreateObject("Wscript.Shell")
 
intReturn=ws.run("cmd /c "&Property("CustomActionData")&"taskDel.bat",0)