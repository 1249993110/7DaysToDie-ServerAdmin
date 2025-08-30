import mitt from 'mitt';

const emitter = mitt();

export const EVENT_TYPES = {
  GAME: {
    CHAT_MESSAGE: 'game:chatMessage',
    CONSOLE_LOG: 'game:consoleLog',
  },
  // UI: {
  //   THEME_CHANGE: 'ui:themeChange',
  // },
};

export default emitter;