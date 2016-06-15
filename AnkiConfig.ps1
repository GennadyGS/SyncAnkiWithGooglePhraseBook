$ankiProfileName = "user1"
@{
    collectionFilePath = "$([Environment]::GetFolderPath("mydocuments"))\Anki\$ankiProfileName\collection.anki2";
    deckName = "Default";
    modelNameTemplate = "Basic"
    webCredentials = @{
        userName = "";
        password = ""
    }
}