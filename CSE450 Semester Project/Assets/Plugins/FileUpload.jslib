mergeInto(LibraryManager.library, {
    OpenFileUploadWeb: function (gameObjectName, methodName) {
        console.log("Calling OnFileUploadWeb");
        var input = document.createElement("input");
        input.type = "file";
        console.log(input);

        input.onchange = function (event) {
            var file = event.target.files[0];
            if (!file) { return; }

            var reader = new FileReader();
            reader.onload = function (e) {
                var contents = new Uint8Array(e.target.result);

                // Create a virtual path
                var path = "/uploaded/" + file.name;
                try {
                    FS.mkdir("/uploaded");
                } catch (e) {} // ignore if exists

                // Write file to upload folder, send message to Unity script
                FS.writeFile(path, contents);
                SendMessage(gameObjectName, methodName, path);
            };
            reader.readAsArrayBuffer(file);
        };
        input.click();
    }
});
