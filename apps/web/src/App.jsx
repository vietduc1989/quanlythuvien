import { Center, Title, Text, Container, Paper } from '@mantine/core';

function App() {
  return (
    <Container size="sm" style={{ marginTop: '10%' }}>
      <Paper shadow="xs" p="xl" withBorder>
        <Center style={{ flexDirection: 'column' }}>
          <Title order={1} c="blue">Quan Ly Thu Vien</Title>
          <Text size="lg" mt="md" c="dimmed">
            He thong quan ly thu vien ONENET Platform.
          </Text>
        </Center>
      </Paper>
    </Container>
  );
}

export default App;
