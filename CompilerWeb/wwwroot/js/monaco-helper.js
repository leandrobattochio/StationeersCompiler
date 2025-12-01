// Monaco Editor resize helper
window.resizeMonacoEditor = function() {
    const editorContainer = document.querySelector('.editor-wrapper');
    if (editorContainer) {
        const width = editorContainer.offsetWidth;
        const height = editorContainer.offsetHeight;
        
        console.log('Forcing Monaco resize to:', width, 'x', height);
        
        // Find all Monaco editor elements and force their size
        const monacoElements = editorContainer.querySelectorAll('.monaco-editor');
        monacoElements.forEach(el => {
            el.style.width = width + 'px';
            el.style.height = height + 'px';
        });
        
        const containers = editorContainer.querySelectorAll('.monaco-editor-container');
        containers.forEach(el => {
            el.style.width = width + 'px';
            el.style.height = height + 'px';
        });
    }
};

// Call on window resize
window.addEventListener('resize', window.resizeMonacoEditor);

// Call after page load
window.addEventListener('load', function() {
    setTimeout(window.resizeMonacoEditor, 100);
    setTimeout(window.resizeMonacoEditor, 500);
    setTimeout(window.resizeMonacoEditor, 1000);
});

