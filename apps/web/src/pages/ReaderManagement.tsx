import { useEffect, useState } from 'react';
import {
  Table, ScrollArea, Button, Group, TextInput, ActionIcon, Modal,
  Stack, Pagination, Title, Badge, Box, LoadingOverlay,
  Menu, Text, Alert, Select
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconSearch, IconPlus, IconEdit, IconUser, IconAlertCircle, IconDotsVertical } from '@tabler/icons-react';
import axiosClient from '../api/apiClient';

interface Reader {
  readerId: string;
  readerCode: string;
  fullName: string;
  phoneNumber: string;
  email?: string;
  address?: string;
  dateOfBirth: string;
  registrationDate: string;
  expiryDate: string;
  status: number;
}

interface ReaderManagementProps {
  autoOpenAdd?: boolean;
  onCloseAutoOpen?: () => void;
}

export default function ReaderManagement({ autoOpenAdd, onCloseAutoOpen }: ReaderManagementProps) {
  const [readers, setReaders] = useState<Reader[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form states
  const [opened, { open, close }] = useDisclosure(false);
  const [editingReader, setEditingReader] = useState<Reader | null>(null);
  const [fullName, setFullName] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [email, setEmail] = useState('');
  const [address, setAddress] = useState('');
  const [registrationDate, setRegistrationDate] = useState('');
  const [expiryDate, setExpiryDate] = useState('');
  const [status, setStatus] = useState<string>('0'); // 0 = Active, 1 = Inactive, 2 = Expired

  const fetchReaders = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await axiosClient.get('/Readers', {
        params: {
          searchTerm: search || undefined,
          status: statusFilter !== null ? Number(statusFilter) : undefined,
          pageIndex: page,
          pageSize: pageSize,
        },
      });

      if (res.data && res.data.success) {
        setReaders(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      } else {
        setError(res.data.message || 'Không thể tải danh sách độc giả.');
      }
    } catch (err: any) {
      console.error(err);
      setError('Đã xảy ra lỗi khi kết nối với máy chủ.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReaders();
  }, [page, search, statusFilter]);

  useEffect(() => {
    if (autoOpenAdd) {
      handleOpenAdd();
      onCloseAutoOpen?.();
    }
  }, [autoOpenAdd]);

  const handleOpenAdd = () => {
    const todayStr = new Date().toISOString().split('T')[0];
    const nextYear = new Date();
    nextYear.setFullYear(nextYear.getFullYear() + 1);
    const nextYearStr = nextYear.toISOString().split('T')[0];

    setEditingReader(null);
    setFullName('');
    setDateOfBirth('');
    setPhoneNumber('');
    setEmail('');
    setAddress('');
    setRegistrationDate(todayStr);
    setExpiryDate(nextYearStr);
    setStatus('0');
    open();
  };

  const handleOpenEdit = async (reader: Reader) => {
    setLoading(true);
    try {
      // Get detailed reader info to ensure we have dateOfBirth etc
      const res = await axiosClient.get(`/Readers/${reader.readerId}`);
      if (res.data && res.data.success) {
        const detail = res.data.data;
        setEditingReader(detail);
        setFullName(detail.fullName);
        setDateOfBirth(detail.dateOfBirth || '');
        setPhoneNumber(detail.phoneNumber);
        setEmail(detail.email || '');
        setAddress(detail.address || '');
        setRegistrationDate(detail.registrationDate || '');
        setExpiryDate(detail.expiryDate || '');
        setStatus(String(detail.status));
        open();
      } else {
        setError(res.data.message || 'Không thể lấy chi tiết độc giả.');
      }
    } catch (err: any) {
      console.error(err);
      setError('Lỗi khi tải thông tin chi tiết độc giả.');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validate 16 years old
    if (dateOfBirth && registrationDate) {
      const dob = new Date(dateOfBirth);
      const reg = new Date(registrationDate);
      let age = reg.getFullYear() - dob.getFullYear();
      const monthDiff = reg.getMonth() - dob.getMonth();
      if (monthDiff < 0 || (monthDiff === 0 && reg.getDate() < dob.getDate())) {
        age--;
      }
      if (age < 16) {
        setError('Độc giả phải từ 16 tuổi trở lên tính đến ngày đăng ký thẻ.');
        return;
      }
    }

    try {
      const payload = {
        fullName,
        dateOfBirth,
        phoneNumber,
        email: email || null,
        address: address || null,
        registrationDate,
        expiryDate,
        status: Number(status),
      };

      if (editingReader) {
        const res = await axiosClient.put(`/Readers/${editingReader.readerId}`, {
          readerId: editingReader.readerId,
          ...payload,
        });
        if (res.data && res.data.success) {
          close();
          fetchReaders();
        } else {
          setError(res.data.message || 'Lỗi cập nhật độc giả.');
        }
      } else {
        const res = await axiosClient.post('/Readers', payload);
        if (res.data && res.data.success) {
          close();
          setPage(1);
          fetchReaders();
        } else {
          setError(res.data.message || 'Lỗi thêm độc giả.');
        }
      }
    } catch (err: any) {
      console.error(err);
      const errMsg = err.response?.data?.message || err.message || 'Không thể lưu thông tin.';
      setError(errMsg);
    }
  };

  const getStatusBadge = (statusCode: number) => {
    switch (statusCode) {
      case 0:
        return <Badge color="green">Đang hoạt động</Badge>;
      case 1:
        return <Badge color="gray">Ngừng hoạt động</Badge>;
      case 2:
        return <Badge color="red">Hết hạn</Badge>;
      default:
        return <Badge color="blue">Khác</Badge>;
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  const rows = readers.map((reader) => (
    <Table.Tr key={reader.readerId}>
      <Table.Td style={{ fontWeight: 600 }}>{reader.readerCode}</Table.Td>
      <Table.Td>
        <Group gap="xs">
          <IconUser size="1rem" style={{ color: '#0ca678' }} />
          <Text size="sm" fw={500}>{reader.fullName}</Text>
        </Group>
      </Table.Td>
      <Table.Td>{reader.phoneNumber}</Table.Td>
      <Table.Td>{reader.email || '-'}</Table.Td>
      <Table.Td>{reader.registrationDate}</Table.Td>
      <Table.Td>{reader.expiryDate}</Table.Td>
      <Table.Td>{getStatusBadge(reader.status)}</Table.Td>
      <Table.Td>
        <Menu shadow="md" width={120}>
          <Menu.Target>
            <ActionIcon variant="subtle" color="gray">
              <IconDotsVertical size="1rem" />
            </ActionIcon>
          </Menu.Target>
          <Menu.Dropdown>
            <Menu.Item leftSection={<IconEdit size="0.8rem" />} onClick={() => handleOpenEdit(reader)}>
              Sửa
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>
      </Table.Td>
    </Table.Tr>
  ));

  return (
    <Box style={{ position: 'relative', minHeight: '400px' }}>
      <LoadingOverlay visible={loading} overlayProps={{ blur: 1 }} />

      <Group justify="space-between" mb="lg">
        <div>
          <Title order={2} fw={700}>Quản lý độc giả</Title>
          <Text size="sm" c="dimmed">Xem, cập nhật, tìm kiếm và tạo mới hồ sơ độc giả thư viện</Text>
        </div>
        <Button onClick={handleOpenAdd} color="teal" leftSection={<IconPlus size="1.2rem" />}>
          Thêm độc giả mới
        </Button>
      </Group>

      <Group mb="md" grow>
        <TextInput
          placeholder="Tìm kiếm theo họ tên hoặc số điện thoại..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          leftSection={<IconSearch size="1rem" />}
        />
        <Select
          placeholder="Lọc theo trạng thái"
          value={statusFilter}
          onChange={(val) => {
            setStatusFilter(val);
            setPage(1);
          }}
          data={[
            { value: '0', label: 'Đang hoạt động' },
            { value: '1', label: 'Ngừng hoạt động' },
            { value: '2', label: 'Hết hạn' },
          ]}
          clearable
        />
      </Group>

      {error && (
        <Alert icon={<IconAlertCircle size="1rem" />} title="Thông báo" color="red" mb="md" withCloseButton onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <ScrollArea style={{ minHeight: 300 }}>
        <Table verticalSpacing="sm" highlightOnHover striped>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Mã độc giả</Table.Th>
              <Table.Th>Họ tên</Table.Th>
              <Table.Th>Số điện thoại</Table.Th>
              <Table.Th>Email</Table.Th>
              <Table.Th>Ngày đăng ký</Table.Th>
              <Table.Th>Ngày hết hạn</Table.Th>
              <Table.Th>Trạng thái</Table.Th>
              <Table.Th style={{ width: 50 }}></Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {rows.length > 0 ? rows : (
              <Table.Tr>
                <Table.Td colSpan={8} style={{ textAlign: 'center' }}>
                  <Text c="dimmed" py="xl">Không tìm thấy độc giả nào.</Text>
                </Table.Td>
              </Table.Tr>
            )}
          </Table.Tbody>
        </Table>
      </ScrollArea>

      {totalPages > 1 && (
        <Group justify="center" mt="xl">
          <Pagination total={totalPages} value={page} onChange={setPage} />
        </Group>
      )}

      <Modal opened={opened} onClose={close} title={editingReader ? 'Cập nhật thông tin độc giả' : 'Đăng ký độc giả mới'} size="md">
        <form onSubmit={handleSubmit}>
          <Stack gap="md">
            <TextInput
              label="Họ và tên"
              required
              placeholder="Nhập họ và tên độc giả"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
            />
            <TextInput
              label="Ngày sinh"
              required
              type="date"
              value={dateOfBirth}
              onChange={(e) => setDateOfBirth(e.target.value)}
            />
            <TextInput
              label="Số điện thoại"
              required
              placeholder="Nhập số điện thoại"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
            />
            <TextInput
              label="Email"
              placeholder="Nhập email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <TextInput
              label="Địa chỉ"
              placeholder="Nhập địa chỉ"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
            />
            <TextInput
              label="Ngày lập thẻ"
              required
              type="date"
              value={registrationDate}
              onChange={(e) => setRegistrationDate(e.target.value)}
            />
            <TextInput
              label="Ngày hết hạn thẻ"
              required
              type="date"
              value={expiryDate}
              onChange={(e) => setExpiryDate(e.target.value)}
            />
            <Select
              label="Trạng thái"
              required
              value={status}
              onChange={(val) => setStatus(val || '0')}
              data={[
                { value: '0', label: 'Đang hoạt động' },
                { value: '1', label: 'Ngừng hoạt động' },
                { value: '2', label: 'Hết hạn' },
              ]}
            />
            <Button type="submit" color="teal" fullWidth mt="md">
              {editingReader ? 'Cập nhật' : 'Đăng ký'}
            </Button>
          </Stack>
        </form>
      </Modal>
    </Box>
  );
}
