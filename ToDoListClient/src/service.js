import axios from 'axios';

// הגדרת כתובת ה-API כ-default
const apiUrl = process.env.REACT_APP_API_KEY;
axios.defaults.baseURL = apiUrl;

// הוספת interceptor לטיפול בשגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    // console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get('/items');
    if (Array.isArray(result.data))
      return result.data;
    else
      return [];
  },

  addTask: async (name) => {
    console.log('addTask', name)
    const result = await axios.post(``, {
      Name: name,
      IsComplete: false
    })
    return { result };
  },
  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete });
    const result = await axios.put(`/${id}`, { isComplete });
    return result.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask', id);
    await axios.delete(`/${id}`);
  }
};