window.focusElement = function(elementId) {
    document.getElementById(elementId)?.focus();
}

window.clickOutsideListener = function(elementId, dotnetHelper) {
    function handleClick(event) {
        var element = document.getElementById(elementId);
        if (element && !element.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('HandleClickOutside');
        }
    }
    document.addEventListener('mousedown', handleClick);
}

window.scrollIntoView = (elementId) => {
    const element = document.getElementById(elementId);
    element?.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'start' });
};

window.keydownInterop = {
    addKeyDownListener: function (dotnetHelper, elementId) {
        const element = document.getElementById(elementId);
        element.addEventListener('keydown', function (event) {

            const keysToPrevent = ['ArrowUp', 'ArrowDown', 'Enter', 'Tab'];
            if(keysToPrevent.includes(event.key)) {
             event.preventDefault();
            }
            
            dotnetHelper.invokeMethodAsync('HandleKeyPressed', event.key);
        });
    }
};