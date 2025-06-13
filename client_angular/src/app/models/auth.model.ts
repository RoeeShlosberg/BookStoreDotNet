export interface UserCredentials {
  username: string;
  password: string;
}

export interface User extends UserCredentials {
  id: number;
  username: string;
  password: string; // Note: In a real application, passwords should not be stored or returned in responses
}

export interface AuthResponse {
  token: string;
  id: number;
  username: string;
}
