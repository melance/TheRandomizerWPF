Public Interface IStorage

    Function ReadFileList() As IEnumerable(Of String)
    Sub WriteFileList(ByVal fileList As IEnumerable(Of String))

End Interface
