import axios, { AxiosError, AxiosResponse } from 'axios';
import { Activity, ActivityFormValues } from '../models/activity';
import { toast } from 'react-toastify';
import { router } from '../router/Routes';
import { store } from '../stores/store';
import { User, UserFormVlaues } from '../models/user';
import { Photo, Profile } from '../models/profile';

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}

axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

axios.defaults.baseURL = 'http://localhost:5000/api';
axios.interceptors.response.use(async response => {
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }
}, (error: AxiosError)=> {
    const { data, status, config } = error.response as AxiosResponse;
    switch (status) {
        case 400:
            if (config.method === 'get' && Object.prototype.hasOwnProperty.call(data.errors, 'id') ){
                router.navigate('/not-found');
            }
            if (data.errors) {
                const modalStoreErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStoreErrors.push(data.errors[key])
                    }
                }
                throw modalStoreErrors.flat();
            } else {
                toast.error(data);
            }
            break;
        case 401:
            toast.error('unauthorized');
            break;
        case 403:
            toast.error('forbidden');
            break;
        case 404:
            router.navigate('/not-found');
            toast.error('not found');
            break;
        case 500:
            store.commonStore.setServerError(data);
            router.navigate('/server-error');
            break;
    }
    return Promise.reject(error);
})
const responseBody = (response: AxiosResponse) => response.data;

const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: <T> (url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T> (url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T> (url: string) => axios.delete<T>(url).then(responseBody),
}

const Activities = {
    list: () => requests.get<Activity[]>('/Activities'),
    details: (id: string) => requests.get<Activity>(`/Activities/${id}`),
    create: (activity: ActivityFormValues) => requests.post<void>('/Activities', activity),
    update: (activity: ActivityFormValues) => requests.put<void>(`/Activities/${activity.id}`, activity),
    delete: (id: string) => requests.del<void>(`/Activities/${id}`),
    attend: (id: string) => requests.post<void>(`/Activities/${id}/attend`, {})

}

const Profiles = {
    get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
    uploadPhoto: (file: Blob) => {
        let formData = new FormData();
        formData.append('File', file);
        return axios.post<Photo>('photos', formData, {
            headers: {'Content-Type': 'multipart/form-data'}
        })
    },

    setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
    deletePhoto: (id: string) => requests.del(`/photos/${id}`)

}

const Account = {

    current: () => requests.get<User>('/account'),
    login: (user: UserFormVlaues) => requests.post<User>('/account/login', user),
    register: (user: UserFormVlaues) => requests.post<User>('/account/register', user)
}

const agent = {
    Activities,
    Account,
    Profiles
}

export default agent;