// QUAN-20260601-1634
import { Button, Text, Box } from '@mantine/core';
import { useEffect, useState } from 'react';
import axiosClient from './api/apiClient';

function App() {
  const [healthStatus, setHealthStatus] = useState<string>('Checking...');
  const [errorStatus, setErrorStatus] = useState<string | null>(null);

  useEffect(() => {
    const fetchHealth = async () => {
      try {
        const response = await axiosClient.get('/HealthCheck');
        setHealthStatus(response.data.message || 'API is healthy.');
      } catch (error: any) {
        setHealthStatus(`API is unhealthy: ${error.message}`);
        console.error('Health check failed:', error);
      }
    };
    fetchHealth();
  }, []);

  const triggerError = async () => {
    try {
      await axiosClient.get('/HealthCheck/error');
      setErrorStatus('Error endpoint did not return an error (unexpected).');
    } catch (error: any) {
      if (error.response && error.response.data) {
        setErrorStatus(`API Error: ${error.response.data.message}`);
      } else {
        setErrorStatus(`Request Error: ${error.message}`);
      }
      console.error('Error endpoint test failed:', error);
    }
  };

  return (
    <Box p="md">
      <Text size="xl" fw={700}>Welcome to ONENET Platform</Text>
      <Text mt="md">Backend Health: {healthStatus}</Text>
      <Button mt="md" onClick={triggerError} color="red">Trigger Test Error on API</Button>
      {errorStatus && <Text color="red" mt="md">Last API Error: {errorStatus}</Text>}
      <Text mt="lg">Start building your application!</Text>
    </Box>
  );
}

export default App;