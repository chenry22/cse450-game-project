mergeInto(LibraryManager.library, {
    QuitGameWebGl: function(){
        try{
            if (UnityInstance && UnityInstance.Quit)
                UnityInstance.Quit();
        }catch(e){
            console.error(`Could not quit: ${e.message}`);
        }
    }
})